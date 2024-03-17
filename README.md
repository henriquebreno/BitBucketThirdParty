# BitBucket Third Party Authentication Example

This is a sample ASP.NET Core MVC application demonstrating how to integrate third-party authentication with BitBucket using OAuth2.

## Setup

1. **Clone the repository:**

    ```bash
    git clone https://github.com/yourusername/your-repo.git
    cd your-repo
    ```

2. **Configure BitBucket OAuth2 Credentials:**

    You need to obtain OAuth2 credentials (Client ID and Client Secret) from BitBucket. Follow these steps:

    - Go to your BitBucket settings and navigate to "OAuth consumers" under "Access Management".
    - Create a new OAuth consumer.
    - Set the callback URL to `https://localhost:7269/account/callback` or adjust it according to your setup.
    - Once you have the Client ID and Client Secret, add them to your `appsettings.json` file under the `Bitbucket` section.

3. **Run the Application:**

    You can run the application using the following commands:

    ```bash
    dotnet restore
    dotnet run
    ```

    The application will be accessible at `https://localhost:7269`.

## Usage

- Visit the `/account/login` endpoint to initiate the login process using BitBucket.
- After successful authentication, you will be redirected back to the application.
- You can access user repositories by visiting the `/account/callback` endpoint, where you will be able to see a list of repositories associated with the authenticated user.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

