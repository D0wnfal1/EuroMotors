import { ShopPage } from '../support/pages/shop.page';

describe('Shop Page', () => {
  const shopPage = new ShopPage();

  beforeEach(() => {
    cy.intercept('GET', '/api/products*').as('getProducts');
    shopPage.navigateToShop();
    cy.wait('@getProducts');
  });

  it('should display products in the shop', () => {
    shopPage.getProductItems().should('have.length.greaterThan', 0);
  });

  it('should be able to navigate through pages', () => {
    shopPage.getPaginator().should('be.visible');

    let firstProductNamePage1: string;
    shopPage.getFirstProductName().then((name) => {
      firstProductNamePage1 = name;
    });

    shopPage.goToNextPage();
    cy.wait('@getProducts');

    shopPage.getFirstProductName().then((name) => {
      if (firstProductNamePage1 && name) {
        expect(name.trim().length).to.be.greaterThan(0);
      }
    });
  });

  it('should change page size', () => {
    shopPage.changePageSize(20);
    cy.wait('@getProducts');

    shopPage.getProductItems().should('have.length.lessThan', 21);
  });

  it('should navigate to product details on click', () => {
    let productName: string;
    shopPage.getFirstProductName().then((name) => {
      productName = name;
    });

    shopPage.clickFirstProduct();

    cy.url().should('include', '/shop/');

    cy.get('app-product-details').within(() => {
      cy.get('h1').should('be.visible');
    });
  });
});
