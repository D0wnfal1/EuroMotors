declare global {
  namespace Cypress {
    interface Chainable {
      xpath(selector: string): Chainable<JQuery<HTMLElement>>;
    }
  }
}

export class AppPage {
  navigateTo(url: string = ''): void {
    cy.visit(url);
  }

  getTitleText(): Cypress.Chainable<string> {
    return cy.get('app-root app-header .logo').invoke('text');
  }

  getElementBySelector(
    selector: string
  ): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.get(selector);
  }

  getElementByXPath(xpath: string): Cypress.Chainable<JQuery<HTMLElement>> {
    return cy.xpath(xpath);
  }

  waitForElementToBeVisible(
    elem: Cypress.Chainable<JQuery<HTMLElement>>
  ): Cypress.Chainable<JQuery<HTMLElement>> {
    return elem.should('be.visible');
  }

  clickElement(selector: string): void {
    cy.get(selector).click();
  }

  sendKeys(selector: string, text: string): void {
    cy.get(selector).clear().type(text);
  }

  getCurrentUrl(): Cypress.Chainable<string> {
    return cy.url();
  }

  getElementText(selector: string): Cypress.Chainable<string> {
    return cy.get(selector).invoke('text');
  }
}
