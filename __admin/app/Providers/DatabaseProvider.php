<?php

namespace App\Providers;

use App\Services\DatabaseService;
use Illuminate\Support\ServiceProvider;

class DatabaseProvider extends ServiceProvider
{
    /**
     * Register services.
     *
     * @return void
     */
    public function register()
    {
        $this->app->singleton('App\Services\DatabaseService', function ($app) {

            return new DatabaseService(
                env('DB_HOST'),
                env('DB_USERNAME'),
                env('DB_PASSWORD'),
                env('DB_DATABASE')
            );
        });
    }

    /**
     * Bootstrap services.
     *
     * @return void
     */
    public function boot()
    {
        //
    }
}
