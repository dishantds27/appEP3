pipeline {
    agent any

    environment {
        APP_NAME = "DotNetCoreSqlDb"
        APP_DIR = "/var/www/dotnetapp"
        APP_SERVICE = "dotnetapp.service"
        DEPLOY_HOST = "192.168.56.10"
        DEPLOY_USER = "vagrant"
        SSH_KEY = "/var/lib/jenkins/.ssh/ops_id_ed25519"
        DB_HOST = "192.168.56.11"
        DB_NAME = "ApplicationDB"
        DB_PASSWORD = "P@ssw0rd123!"
    }

    stages {

        stage('Build') {
            steps {
                echo 'Building application...'
                sh 'dotnet build --configuration Release'
            }
        }

        stage('Test') {
            steps {
                echo 'Running tests...'
                sh 'dotnet test --configuration Release --no-build || true'
            }
        }

        stage('Publish') {
            steps {
                echo 'Publishing application...'
                sh """
                    rm -rf ./publish
                    dotnet publish DotNetCoreSqlDb/DotNetCoreSqlDb.csproj --configuration Release --output ./publish
                """
            }
        }

        stage('Deploy') {
            steps {
                echo 'Deploying to app server...'
                sh """
                    set -e

                    ssh -i "$SSH_KEY" -o StrictHostKeyChecking=no \
                        "$DEPLOY_USER@$DEPLOY_HOST" \
                        "sudo mkdir -p $APP_DIR && sudo chown $DEPLOY_USER:$DEPLOY_USER $APP_DIR"

                    scp -i "$SSH_KEY" -o StrictHostKeyChecking=no -r \
                        ./publish/* "$DEPLOY_USER@$DEPLOY_HOST:$APP_DIR/"

                    ssh -i "$SSH_KEY" -o StrictHostKeyChecking=no \
                        "$DEPLOY_USER@$DEPLOY_HOST" \
                        "sudo systemctl restart $APP_SERVICE"
                """
         }
        }
    }

    post {
        success {
            echo 'Pipeline succesvol afgerond!'
        }
        failure {
            echo 'Pipeline gefaald!'
        }
    }
}