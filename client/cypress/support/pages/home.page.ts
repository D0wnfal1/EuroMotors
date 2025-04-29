import { AppPage } from '../app.page';

export class HomePage extends AppPage {
  navigateToHome(): void {
    this.navigateTo('/');
  }

  getCarSelectionComponent(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.get('app-car-selection');
  }

  getCarBrandsSection(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.get('h2:contains("Car Brands")').parent();
  }

  getBrandItems(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.get('button[routerLink^="/brand/"]');
  }

  getCategoriesSection(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.get('h2:contains("Product Categories")').parent();
  }

  getCategoryCards(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.get('mat-card');
  }

  getViewAllBrandsButton(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.get(
      'button:contains("All Brands"), button:contains("Show less")'
    );
  }

  getProductSlider(): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.get('app-product-slider');
  }

  clickViewAllBrandsButton(): void {
    this.getViewAllBrandsButton().click({ force: true });
  }

  clickOnBrand(brandIndex: number = 0): void {
    this.getBrandItems().eq(brandIndex).click({ force: true });
  }

  clickOnCategory(categoryIndex: number = 0): void {
    this.getCategoryCards()
      .eq(categoryIndex)
      .find('button:contains("Explore Category")')
      .click({ force: true });
  }

  clickOnCategoryByName(categoryName: string): void {
    this.getCategoryCards()
      .contains(categoryName)
      .parents('mat-card')
      .find('button:contains("Explore Category")')
      .click({ force: true });
  }

  shouldDisplayCarBrands(minCount: number = 1): void {
    this.getBrandItems().should('have.length.at.least', minCount);
  }

  shouldDisplayCategories(minCount: number = 1): void {
    this.getCategoryCards().should('have.length.at.least', minCount);
  }

  shouldDisplayCarSelection(): void {
    this.getCarSelectionComponent().should('be.visible');
  }

  shouldDisplayProductSlider(): void {
    this.getProductSlider().should('be.visible');
  }
}
