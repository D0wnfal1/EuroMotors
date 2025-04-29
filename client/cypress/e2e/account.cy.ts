import { AccountPage } from '../support/pages/account.page';
import { HomePage } from '../support/pages/home.page';

describe('Account Pages', () => {
  const accountPage = new AccountPage();
  const homePage = new HomePage();

  const testUser = {
    email: `test${Date.now()}@example.com`,
    password: 'Test123!',
    firstName: 'Test',
    lastName: 'User',
  };

  beforeEach(() => {
    cy.intercept('POST', '/api/auth/login').as('loginRequest');
    cy.intercept('POST', '/api/auth/register').as('registerRequest');
  });

  describe('Login Page', () => {
    beforeEach(() => {
      accountPage.navigateToLogin();
    });

    it('should show login form', () => {
      accountPage.shouldShowLoginForm();
    });

    it('should show validation errors with empty input', () => {
      accountPage.getErrorMessage().should('not.exist');

      accountPage.getEmailInput().click().blur();
      accountPage.getPasswordInput().click().blur();

      accountPage.shouldShowValidationErrors();
    });

    it('should validate email format', () => {
      accountPage.getEmailInput().type('invalid-email');
      accountPage.getPasswordInput().click();

      cy.get('mat-error').should('contain.text', 'valid email');
    });
  });
});
