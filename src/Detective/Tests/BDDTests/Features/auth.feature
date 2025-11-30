Feature: Authentication with 2FA
End-to-end авторизация через логин/пароль, 2FA, блокировку и восстановление.

    Scenario: Successful login via 2FA
        Given a technical user exists
        When I login with correct username and password
        Then a 2FA code is sent to my email
        When I confirm 2FA with the correct code
        Then a JWT token is returned

    Scenario: Block after 3 incorrect 2FA attempts
        Given a technical user exists
        When I login with correct username and password
        Then a 2FA code is sent to my email
        When I confirm 2FA with wrong code three times
        Then my password is automatically changed
        And I receive a new password via email

    Scenario: Login using temporary password after block
        Given a technical user exists
        And I already triggered 2FA block
        When I login using the temporary password
        Then a 2FA code is sent to my email
        When I confirm 2FA with the correct code
        Then a JWT token is returned