import 'cypress-xpath';

Cypress.Commands.add('navigateToShop', () => {
  cy.visit('/shop');
  cy.wait('@getProducts');
});

Cypress.Commands.add('addProductToCart', (index = 0) => {
  cy.get('app-product-item')
    .eq(index)
    .find('button:contains("Add to cart")')
    .click({ force: true });
  cy.wait('@addToCart');
});

Cypress.Commands.add('navigateToCart', () => {
  cy.visit('/cart');
});

Cypress.Commands.add('checkEmptyCart', () => {
  cy.get('app-cart-empty-message').should('be.visible');
  cy.get('app-cart-empty-message').should('contain.text', 'Your cart is empty');
});

Cypress.Commands.add('checkCartItemsCount', (count) => {
  cy.get('app-cart-item').should('have.length', count);
});

Cypress.Commands.add('waitForElementToBeVisible', (selector) => {
  cy.get(selector, { timeout: 10000 }).should('be.visible');
});

Cypress.Commands.add('waitForPageLoad', () => {
  cy.window()
    .should('have.property', 'document')
    .and('have.property', 'readyState')
    .and('eq', 'complete');
});

declare global {
  namespace Cypress {
    interface Chainable {
      navigateToShop(): void;
      addProductToCart(index?: number): void;
      navigateToCart(): void;
      checkEmptyCart(): void;
      checkCartItemsCount(count: number): void;
      waitForElementToBeVisible(
        selector: string
      ): Chainable<JQuery<HTMLElement>>;
      waitForPageLoad(): void;
      xpath(selector: string): Chainable<JQuery<HTMLElement>>;
    }
  }
}
