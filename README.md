# EchoStudy API

The backend of echostudy.com

## Description

This project was made in Spring 2022 and Fall 2022 as a senior project at the University of Utah. It uses a SQL database to store all of our users' information and uses Amazon services such as S3 and Amazon Polly. Our REST API provides endpoints to communicate with our database and amazon services as needed. User accounts are specifically handled with .NET Identity for security and communicates these to the front-end using JWT. Documentation is outlined on our Swagger page which can be accessed on our live site at https://api.echoStudy.com/swagger or by running the project locally. The API and its frontend counterpart are hosted on an AWS EC2 instance at https://api.echostudy.com and https://www.echostudy.com respectively.

## Change Log

* 0.4
    * AuthController created with endpoints for user accounts
    * JWT Tokens implemented
    * Refresh token rotation implemented

* 0.3
    * File naming system for audio files fixed allowing all characters
    * Audio files will now always be the same voice for our supported languages

* 0.22
    * Swagger page now has proper documentation for Cards, Decks, and DeckCategories
    * Miscellaneous inconsistencies and errors in Cards, Decks, and DeckCategories bugfixed

* 0.21
    * API for Cards, Decks, and DeckCategories overhauled with functional and working controllers
    * All cards now have Amazon Polly audio files on S3 
    * Swagger documentation added for Cards, Decks, and DeckCategories

* 0.20
    * API for Cards, Decks, and DeckCategories overhauled with functional and working controllers
    * All cards now have Amazon Polly audio files
    * Endpoints when hit will now give a reasonable response

* 0.11
    * Basic API implemented for Cards, Decks, and DeckCategories.
    * Code created that is functional for Amazon Polly and S3 buckets 

* 0.10
    * EchoStudy and Identity databases created
	* Models created for Decks, Cards, DeckCategories, and Sessions created.
    * Created controllers for all models
    * Swagger added to the project


## Authors

Mason Austin, Derrick Lee, Jason Liu, and Jon Strode.
