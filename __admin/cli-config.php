<?php

use Doctrine\ORM\Tools\Console\ConsoleRunner;

require_once 'vendor/autoload.php';

$dotenv = \Dotenv\Dotenv::create('.');
$environment = $dotenv->load();

// replace with mechanism to retrieve EntityManager in your app
$dbService = new \App\Services\DatabaseService(
    $environment['DB_HOST'],
    $environment['DB_USERNAME'],
    $environment['DB_PASSWORD'],
    $environment['DB_DATABASE']);


return ConsoleRunner::createHelperSet($dbService->getEntityManager());