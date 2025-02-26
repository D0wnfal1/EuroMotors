FROM node:22-alpine

WORKDIR /src/app

COPY . /src/app/

RUN npm install -g @angular/cli@17
RUN npm ci
COPY . .

EXPOSE 4200
CMD ["ng", "serve", "--host", "0.0.0.0"]