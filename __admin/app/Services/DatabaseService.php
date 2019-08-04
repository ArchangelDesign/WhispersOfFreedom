<?php


namespace App\Services;


use Doctrine\ORM\EntityManager;
use Doctrine\ORM\OptimisticLockException;
use Doctrine\ORM\ORMException;
use Doctrine\ORM\Tools\Setup;

class DatabaseService
{
    /**
     * @var EntityManager
     */
    private $entityManager;

    /**
     * DatabaseService constructor.
     * @param string $host
     * @param string $user
     * @param string $password
     * @param string $dbName
     * @throws ORMException
     */
    public function __construct(string $host, string $user, string $password, string $dbName)
    {
        // app_path() cannot be used here because it prevents
        // Doctrine CLI from loading properly
        // $paths = [app_path('Entities')];
        $paths = [dirname(__FILE__ , 2) . '/Entities' ];
        $isDevMode = true;
        $dbParams = array(
            'driver' => 'pdo_mysql',
            'host' => $host,
            'user' => $user,
            'password' => $password,
            'dbname' => $dbName,
        );
        $config = Setup::createAnnotationMetadataConfiguration($paths, $isDevMode);
        $this->entityManager = EntityManager::create($dbParams, $config);
    }

    /**
     * @return EntityManager
     */
    public function getEntityManager(): EntityManager {
        return $this->entityManager;
    }

    /**
     * Generic method for returning entity from persistence layer
     *
     * @param string $entity
     * @param $identifier
     * @return object|null
     * @throws ORMException
     * @throws OptimisticLockException
     * @throws \Doctrine\ORM\TransactionRequiredException
     */
    public function fetchEntity(string $entity, $identifier) {
        return $this->entityManager->find($entity, $identifier);
    }

    /**
     * @param $entity
     * @throws ORMException
     */
    public function persist($entity)
    {
        $this->getEntityManager()->persist($entity);
    }

    /**
     * @throws ORMException
     * @throws OptimisticLockException
     */
    public function flush() {
        $this->getEntityManager()->flush();
    }
}