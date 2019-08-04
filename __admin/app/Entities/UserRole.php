<?php


namespace App\Entities;


/**
 * Class UserRole
 * @package App\Entities
 * @Entity
 * @Table(name="user_roles")
 */
class UserRole
{
    /**
     * @var integer
     * @Column(type="integer")
     * @Id
     * @GeneratedValue
     */
    protected $id;

    /**
     * @var string
     * @Column(type="string", length=120, nullable=false, unique=true)
     */
    protected $name;

    /**
     * @var UserRole
     * @ManyToOne(targetEntity="App\Entities\UserRole", cascade={})
     * @JoinColumn(name="parent_id")
     */
    protected $parent;

    /**
     * @return int
     */
    public function getId(): int
    {
        return $this->id;
    }

    /**
     * @return string
     */
    public function getName(): string
    {
        return $this->name;
    }

    /**
     * @param string $name
     * @return UserRole
     */
    public function setName(string $name): UserRole
    {
        $this->name = $name;
        return $this;
    }

    /**
     * @return UserRole
     */
    public function getParent(): UserRole
    {
        return $this->parent;
    }

    /**
     * @param UserRole $parent
     * @return UserRole
     */
    public function setParent(?UserRole $parent): UserRole
    {
        $this->parent = $parent;
        return $this;
    }


}