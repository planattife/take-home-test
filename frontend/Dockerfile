FROM node:18 AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build --output-path=dist/temp

FROM nginx:alpine
COPY --from=build /app/dist/temp/browser /usr/share/nginx/html
EXPOSE 80
