<?php


namespace App\Entities;

/**
 * Class NewsletterRecipient
 * @package App\Entities
 * @Entity
 * @Table(name="newsletter_recipients")
 */
class NewsletterRecipient
{
    /**
     * @var integer
     * @Id
     * @GeneratedValue
     */
    protected $id;

    /**
     * @var string
     * @Column(type="string", length=250, nullable=false, unique=true)
     */
    protected $email;

    /**
     * @var integer
     * @Column(type="datetime", nullable=false, name="created_date")
     */
    protected $createdDate;

    /**
     * NewsletterRecipient constructor.
     */
    public function __construct()
    {
        $this->createdDate = time();
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
     * @return NewsletterRecipient
     */
    public function setEmail(string $email): NewsletterRecipient
    {
        $this->email = $email;
        return $this;
    }

    /**
     * @return int
     */
    public function getCreatedDate(): int
    {
        return $this->createdDate;
    }

    /**
     * @param int $createdDate
     * @return NewsletterRecipient
     */
    public function setCreatedDate(int $createdDate): NewsletterRecipient
    {
        $this->createdDate = $createdDate;
        return $this;
    }

    /**
     * @return int
     */
    public function getId(): int
    {
        return $this->id;
    }


}