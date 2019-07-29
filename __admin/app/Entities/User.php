<?php


namespace App\Entities;

/**
 * Class User
 * @package App\Entities
 * @Entity
 * @Table(name="Users")
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


}