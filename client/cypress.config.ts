import { defineConfig } from 'cypress';

export default defineConfig({
  projectId: 'jvg29w',
  e2e: {
    baseUrl: 'https://localhost:4200',
    setupNodeEvents(on, config) {},
  },

  component: {
    devServer: {
      framework: 'angular',
      bundler: 'webpack',
    },
  },
});
