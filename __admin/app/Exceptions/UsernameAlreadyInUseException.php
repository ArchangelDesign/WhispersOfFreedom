<?php


namespace App\Exceptions;


use Exception;

class UsernameAlreadyInUseException extends Exception
{
    public function __construct()
    {
        parent::__construct("Username already in use.");
    }

}