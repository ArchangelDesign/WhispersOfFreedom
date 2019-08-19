<?php

namespace App\Http\Controllers;

use App\Exceptions\UserAlreadyRegisteredException;
use App\Exceptions\UsernameAlreadyInUseException;
use App\Providers\DatabaseProvider;
use App\Services\DatabaseService;
use App\Services\UserService;
use Doctrine\ORM\NonUniqueResultException;
use Doctrine\ORM\OptimisticLockException;
use Doctrine\ORM\ORMException;
use Exception;
use http\Env\Response;
use Illuminate\Contracts\View\Factory;
use Illuminate\Http\JsonResponse;
use Illuminate\Http\RedirectResponse;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Redirect;
use Illuminate\View\View;

class UserController extends Controller
{
    public function doLogin(UserService $userService, Request $request)
    {

//        $db->registerUser('');
//        $db->fetchUserByEmail('');
    }

    /**
     * @param UserService $userService
     * @param Request $request
     * @return Factory|RedirectResponse|View
     */
    public function doRegister(UserService $userService, Request $request)
    {
        $validated = $request->validate([
            'email' => 'required|email',
            'password' => 'required|regex:/(?=.{8,})(?=.*[0-9])(?=.*[A-Z])/i',
            'username' => 'required|Min:4'
        ]);

        $username = $validated['username'];
        $password = $validated['password'];
        $email = $validated['email'];

        try {
            $userService->registerUser($email, $username, $password);
        } catch (UserAlreadyRegisteredException $e) {
            return Redirect::route('register')->withErrors([
                'Already registered' => 'Email has already been registered'
            ]);
        } catch (UsernameAlreadyInUseException $e) {
            return Redirect::route('register')->withErrors([
                'username' => 'Username is already in use.'
            ])->with('email', $email);
        } catch (NonUniqueResultException $e) {
            return Redirect::route('register')->withErrors([
                'NonUniqueResultException' => $e->getMessage()
            ]);
        } catch (OptimisticLockException $e) {
            return Redirect::route('register')->withErrors([
                'OptimisticLockException' => $e->getMessage()
            ]);
        } catch (ORMException $e) {
            return Redirect::route('register')->withErrors([
                'ORMException' => $e->getMessage()
            ]);
        }
        return view('registration-success');
    }

    /**
     * @param Request $request
     * @param UserService $userService
     * @return JsonResponse
     */
    public function newsletterSignup(Request $request, UserService $userService)
    {
        try {
            $userService->signupForNewsletter(strtolower($request->get('email')));
        } catch (Exception $e) {
            return response()->json(['result' => false, 'message' => $e->getMessage()]);
        }

        return response()->json(['result' => true, 'message' => 'OK']);
    }

    /**
     * @param Request $request
     * @param UserService $userService
     * @return JsonResponse
     * @throws NonUniqueResultException
     */
    public function verifyCredentials(Request $request, UserService $userService)
    {
        if (empty($request->get('email')) || empty($request->get('password')))
            return response()->json([], 400);

        $user = $userService->fetchUserByEmail($request->get('email'));
        if (!$user)
            return response()->json([], 400);

        if (!$userService->verifyPassword($user, $request->get('password')))
            return response()->json([], 400);

        return response()->json([], 200);
    }
}
