
# .Net 5 Catalog Api

I made this project to learn .Net 5. While doing the project, I benefited from Julio Casal's youtube channel.

You can find the docker image of the application here.
https://hub.docker.com/repository/docker/mutluozkurt/catalog

# MongoDB
For the application to work correctly, you must run mongodb over docker as follows.

`docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db -e MONGO_INITDB_ROOT_USERNAME=mongoadmin -e MONGO_INITDB_ROOT_PASSWORD=Pass#word1 --network=net5tutorial mongo
`

If you are going to run it over the docke image, you need to run it as follows.

`docker run -it --rm -p 8080:80 -e MongoDbSettings:Host=mongo -e MongoDbSettings:Password=Pass#word1 --network=net5tutorial mutluozkurt/catalog:v2  
`
