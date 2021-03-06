<?php

/*
|--------------------------------------------------------------------------
| Web Routes
|--------------------------------------------------------------------------
|
| Here is where you can register web routes for your application. These
| routes are loaded by the RouteServiceProvider within a group which
| contains the "web" middleware group. Now create something great!
|
*/

Route::get('/', function () {
    return view('home');
})->name('home');

Route::prefix('user')->group(function () {
    Route::get('/login', function () {
        return view('login');
    })->name('login');
    Route::post('/login', 'UserController@doLogin')->name('do-login');
    Route::post('/register', 'UserController@doRegister')->name('do-register');
    Route::get('/register', function () {
        return view('register');
    })->name('register');
    Route::post('/newsletter/signup', 'UserController@newsletterSignup')->name('newsletter-signup');
});

