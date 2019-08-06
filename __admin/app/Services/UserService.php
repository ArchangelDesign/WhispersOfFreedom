<?php


namespace App\Services;


use App\Entities\User;
use App\Entities\UserRole;
use App\Exceptions\UserAlreadyRegisteredException;
use Doctrine\ORM\NonUniqueResultException;
use Doctrine\ORM\OptimisticLockException;
use Doctrine\ORM\ORMException;

class UserService
{
    /**
     * @var DatabaseService
     */
    private $databaseService;

    /**
     * UserService constructor.
     * @param DatabaseService $db
     */
    public function __construct(DatabaseService $db)
    {
        $this->databaseService = $db;
    }

    private function hashPassword(string $password): string
    {
        return password_hash($password, PASSWORD_BCRYPT);
    }

    private function passwordValid(string $password, string $hash): bool
    {
        return password_verify($password, $hash);
    }

    private function verifyPassword(User $user, string $password): bool
    {
        if (empty($user))
            return false;

        return $this->passwordValid($password, $user->getPassword());
    }

    /**
     * @param string $email
     * @return User|null
     * @throws NonUniqueResultException
     */
    public function fetchUserByEmail(string $email)
    {
        return $this->databaseService
            ->getEntityManager()
            ->createQueryBuilder()
            ->select('u')
            ->from('App\Entities\User', 'u')
            ->where('u.email = :email')
            ->setParameter('email', $email)
            ->getQuery()->getOneOrNullResult();
    }

    /**
     * @param string $email
     * @param string $username
     * @param string $password
     * @throws NonUniqueResultException
     * @throws ORMException
     * @throws UserAlreadyRegisteredException
     * @throws OptimisticLockException
     */
    public function registerUser(string $email, string $username, string $password)
    {
        if ($this->fetchUserByEmail($email))
            throw new UserAlreadyRegisteredException();

        $userEntity = new User();
        $userEntity->setEmail(strtolower($email))
            ->setUsername($username)
            ->setPassword($this->hashPassword($password));

        $this->databaseService->persist($userEntity);
        $this->databaseService->flush();
    }

    /**
     * @param string $name
     * @param UserRole|null $parent
     * @return UserRole
     * @throws ORMException
     */
    public function createRole(string $name, UserRole $parent = null)
    {
        $newRole = new UserRole();
        $newRole
            ->setName($name)
            ->setParent($parent);
        $this->databaseService->persist($newRole);
        $this->databaseService->flush();
        return $newRole;
    }

    /**
     * @param string $name
     * @return mixed
     * @throws NonUniqueResultException
     */
    public function fetchRoleByName(string $name)
    {
        $query = $this->databaseService->getEntityManager()->createQuery(
            "select r from App\Entities\UserRole r "
            . "where r.name = :name"
        )->setParameter('name', $name);

        return $query->getOneOrNullResult();
    }
}