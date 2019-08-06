<?php


namespace App\Exceptions;


use Exception;

class UserAlreadyRegisteredException extends Exception
{
    public function __construct()
    {
        parent::__construct("User already registered.");
    }

}