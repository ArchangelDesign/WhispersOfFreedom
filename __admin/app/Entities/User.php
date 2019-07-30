<?php


namespace App\Entities;

/**
 * Class User
 * @package App\Entities
 * @Entity
 * @Table(name="users")
 */
class User
{
    /**
     * @var int
     * @Id
     * @Column(type="integer")
     * @GeneratedValue
     */
    protected $id;


    /**
     * @var string
     * @Column(type="string", length=120, nullable=false, unique=true)
     */
    protected $username;

    /**
     * @var string
     * @Column(type="string", length=255, nullable=false, unique=true)
     */
    protected $email;

    /**
     * @var int
     * @Column(name="registration_date", type="datetime", options={"default": "CURRENT_TIMESTAMP"}, nullable=false)
     */
    protected $registrationDate;
}