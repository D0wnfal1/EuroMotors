import { AppPage } from '../app.page';

export class AccountPage extends AppPage {
  navigateToLogin(): void {
    this.navigateTo('/account/login');
  }

  navigateToRegister(): void {
    this.navigateTo('/account/register');
  }

  getLoginForm(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.get('form[formGroup="loginForm"]');
  }

  getRegisterForm(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.get('form[formGroup="registerForm"]');
  }

  getEmailInput(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.get('input[formControlName="email"]');
  }

  getPasswordInput(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.get('input[formControlName="password"]');
  }

  getFirstNameInput(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.get('input[formControlName="firstName"]');
  }

  getLastNameInput(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.get('input[formControlName="lastName"]');
  }

  getSubmitButton(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.get('button[type="submit"]');
  }

  getErrorMessage(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.get('mat-error');
  }

  login(email: string, password: string): void {
    this.getEmailInput().type(email);
    this.getPasswordInput().type(password);
    this.getSubmitButton().click({ force: true });
  }

  register(
    email: string,
    firstName: string,
    lastName: string,
    password: string
  ): void {
    this.getEmailInput().type(email);
    this.getFirstNameInput().type(firstName);
    this.getLastNameInput().type(lastName);
    this.getPasswordInput().type(password);
    this.getSubmitButton().click({ force: true });
  }

  shouldShowLoginForm(): void {
    cy.get('h1').should('contain.text', 'Login');
    this.getEmailInput().should('be.visible');
    this.getPasswordInput().should('be.visible');
  }

  shouldShowRegisterForm(): void {
    cy.get('h1').should('contain.text', 'Register');
    this.getEmailInput().should('be.visible');
    this.getFirstNameInput().should('be.visible');
    this.getLastNameInput().should('be.visible');
    this.getPasswordInput().should('be.visible');
  }

  shouldShowValidationErrors(): void {
    this.getErrorMessage().should('be.visible');
  }

  shouldBeLoggedIn(): void {
    cy.get('button:contains("Logout")').should('be.visible');
  }
}
