import { HomePage } from '../support/pages/home.page';
import { ShopPage } from '../support/pages/shop.page';

describe('Home Page', () => {
  const homePage = new HomePage();

  beforeEach(() => {
    cy.intercept('GET', '/api/products*').as('getProducts');

    homePage.navigateToHome();
  });

  it('should display the car selection component', () => {
    homePage.shouldDisplayCarSelection();
  });

  it('should display product categories section', () => {
    homePage.getCategoriesSection().should('be.visible');
    homePage.shouldDisplayCategories(1);
  });

  it('should display product slider section', () => {
    homePage.shouldDisplayProductSlider();
  });
});
