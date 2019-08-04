<?php


namespace App\Services;


use App\Entities\UserRole;
use Doctrine\ORM\NonUniqueResultException;
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

    public function fetchUserByEmail(string $email)
    {
        $query = $this->databaseService->getEntityManager()->createQuery(
            "select u from App\Entities\User u "
            . "where u.email like :email"
        )->setParameter('email', $email);
        $result = $query->getResult();
        var_dump($result);
    }

    public function registerUser(string $email, string $username)
    {

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