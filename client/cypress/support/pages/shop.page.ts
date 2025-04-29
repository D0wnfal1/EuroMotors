import { AppPage } from '../app.page';

export class ShopPage extends AppPage {
  navigateToShop(): void {
    this.navigateTo('/shop');
  }

  getProductItems(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.get('app-product-item');
  }

  getPaginator(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.get('mat-paginator');
  }

  getSortButton(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.get('.sort-button, [aria-label="Sort"]', { timeout: 10000 });
  }

  getProductGrid(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.get('.grid');
  }

  getFirstProductName(): Cypress.Chainable<string> {
    return this.getProductItems().first().find('.font-bold').invoke('text');
  }

  getFirstProductPrice(): Cypress.Chainable<string> {
    return this.getProductItems().first().find('.font-semibold').invoke('text');
  }

  getFirstProductImage(): Cypress.Chainable<JQuery<HTMLImageElement>> {
    return this.getProductItems().first().find('img');
  }

  clickFirstProduct(): void {
    this.getProductItems().first().click();
  }

  addFirstProductToCart(): void {
    this.getProductItems()
      .first()
      .find('button:contains("Add to cart")')
      .click({ force: true });
  }

  clickSortButton(): void {
    this.getSortButton().click({ force: true });
  }

  selectSortOption(option: string): void {
    cy.get('mat-selection-list').contains(option).click({ force: true });
  }

  goToNextPage(): void {
    cy.get('button.mat-mdc-paginator-navigation-next').click({ force: true });
  }

  goToPreviousPage(): void {
    cy.get('button.mat-mdc-paginator-navigation-previous').click({
      force: true,
    });
  }

  changePageSize(size: number): void {
    cy.get('mat-paginator div.mat-mdc-select-trigger').click({ force: true });
    cy.get('mat-option').contains(size.toString()).click({ force: true });
  }
}
