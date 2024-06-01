# Imatex

"Optical Character Recognition". It is a technology that allows the conversion of different types of documents, such as scanned documents, images or documents captured by a camera, into editable text.

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Installation](#installation)
- [Usage](#usage)
- [Deployment](#deployment)
- [Notes](#notes)

## Features

- Extract Image From Documents
- Extract Text From Images
- Download Youtube Video
- Download Tiktok Video

## Tech Stack

- .NET 8+
- <a href="https://tesseract-ocr.github.io/">Tesseract 5+</a>
- Visual Studio 2022+
  
## Installation

To get started with the project, follow these steps:

<p>Restore all packages:</p>

1. Clone the repository:
    ```sh
    git clone https://github.com/jmatoso/Imatex.git
    cd Imatex
    ```

2. Restore dependencies:
    ```sh
    dotnet restore
    ```
3. Set up environment variables:
    Change the <code>appsettings.json</code> file <code>Imatex.Web</code> and add the following variables:
    ```sh
    "GoogleTagId": "${GoogleTagId}",
    "TikTokOptions": {
        "TikTokDownloadApi": "${TikTokApi}",
        ...
    }
    ```

## Usage

To start the development server, set <code>Imatex.Web</code> as startup project or run:

```sh
dotnet run
```

## Deployment

The site is hosted on Render and the database on Aiven. Follow these steps for deployment:

1. Set up your Render account and create a new web service.
2. Link your GitHub repository to Render.
3. Add the required environment variables in Render's dashboard.
4. Deploy the application directly from the Render dashboard.

<p>or</p>

<a href="https://render.com/deploy?repo=https://github.com/JMatoso/Imatex">
    <img src="https://render.com/images/deploy-to-render-button.svg" alt="Deploy to Render" />
</a>

## Notes
Render free server might not be enough to run the app properly, but you can still try. Good Luck! :)
