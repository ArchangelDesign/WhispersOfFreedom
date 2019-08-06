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
     * @var string
     * @Column(type="string", length=255, nullable=false)
     */
    protected $password;

    /**
     * @var array[UserRole]
     * @ManyToMany(targetEntity="App\Entities\UserRole", cascade={})
     * @JoinTable(name="users_to_roles")
     */
    protected $roles;

    /**
     * @var int
     * @Column(name="registration_date", type="datetime", nullable=false)
     */
    protected $registrationDate;

    /**
     * User constructor.
     */
    public function __construct()
    {
        $this->registrationDate = now();
    }


    /**
     * @return string
     */
    public function getUsername(): string
    {
        return $this->username;
    }

    /**
     * @param string $username
     * @return User
     */
    public function setUsername(string $username): User
    {
        $this->username = $username;
        return $this;
    }

    /**
     * @return string
     */
    public function getEmail(): string
    {
        return $this->email;
    }

    /**
     * @param string $email
     * @return User
     */
    public function setEmail(string $email): User
    {
        $this->email = $email;
        return $this;
    }

    /**
     * @return string
     */
    public function getPassword(): string
    {
        return $this->password;
    }

    /**
     * @param string $password
     * @return User
     */
    public function setPassword(string $password): User
    {
        $this->password = $password;
        return $this;
    }

    /**
     * @return array
     */
    public function getRoles(): array
    {
        return $this->roles;
    }

    /**
     * @param array $roles
     * @return User
     */
    public function setRoles(array $roles): User
    {
        $this->roles = $roles;
        return $this;
    }

    /**
     * @return int
     */
    public function getRegistrationDate(): int
    {
        return $this->registrationDate;
    }

    /**
     * @param int $registrationDate
     * @return User
     */
    public function setRegistrationDate(int $registrationDate): User
    {
        $this->registrationDate = $registrationDate;
        return $this;
    }

}