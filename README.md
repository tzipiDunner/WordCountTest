# Word Count Application

This Word Count Application is a .NET Core API designed to process text files by counting the occurrences of each word and reporting how many times each word appears.
These results are uploaded to an AWS S3 bucket, and the API allows retrieval of these results from specified files within the bucket. 
This setup demonstrates a practical application of cloud storage integration with .NET Core APIs.

## Features

- **Count Words**: Analyze text files to count word occurrences.
- **Save to AWS S3**: Automatically uploads word count results to an AWS S3 bucket.
- **Retrieve Results**: Allows fetching of word count results directly from AWS S3 using the file key.

## Getting Started

Follow these instructions to get a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

What you need to install the software:

- .NET Core 3.1 SDK or later: [Download .NET Core](https://dotnet.microsoft.com/download)
- AWS CLI configured on your machine: [Installing AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/install-cliv2.html)
- An AWS account with access to S3: [AWS](https://aws.amazon.com/)

### Installation

Here are the steps to set up your project locally.

1. **Clone the Repository**

   git clone https://github.com/tzipiDunner/WordCountTest.git

2. **Navigate to the Project Directory**

    cd wordCountApp

3. **Restore Dependencies**

     dotnet restore

5. **Configuration**

    Update appsettings.json with your AWS settings:
    {
    "AWS": {
      "BucketName": "cloudguard-wordcount",
      "KeyPrefix": "wordcounts/",
      "Region": "eu-north-1"
      }
    }

5.**Run the Application**

    dotnet run

### Usage
Upload a Text File
Endpoint: POST /api/WordCount/upload
Form Data: Include the text file in the request with the key file.
Success Response: JSON object detailing word counts.
Retrieve Word Count Results
Endpoint: GET /api/WordCount/retrieve/{fileKey}
Parameter: fileKey is the S3 object key.
Success Response: JSON object of the word count results.






