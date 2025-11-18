FROM fabiocicerchia/nginx-lua:alpine

RUN rm -rf /etc/nginx/conf.d/*

COPY nginx.conf /etc/nginx/nginx.conf
COPY conf.d/server.conf /etc/nginx/conf.d/

RUN mkdir -p /var/www/weblab/static \
    /var/www/weblab/management \
    /var/www/weblab/legacy \
    /var/www/weblab/documentation

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]