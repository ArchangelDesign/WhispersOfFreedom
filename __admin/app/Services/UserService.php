<?php


namespace App\Services;


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
    public function __construct(DatabaseService $db) {
        $this->databaseService = $db;
    }

    public function fetchUserByEmail(string $email) {
        $query = $this->databaseService->getEntityManager()->createQuery(
            "select u from App\Entities\User u "
            . "where u.email like :email"
        )->setParameter('email', $email);
        $result = $query->getResult();
        var_dump($result);
    }
}