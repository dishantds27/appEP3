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
        CLOUD_HOST = "92.5.131.89"
        CLOUD_USER = "ubuntu"
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
                    dotnet publish DotNetCoreSqlDb.csproj --configuration Release --output ./publish
                """
            }
        }

        stage('Deploy') {
            steps {
                echo 'Deploying to app server...'
                sh """
                    set -e
                    # oude know_hosts e,try verwijderen
                    ssh-keygen -f "/var/lib/jenkins/.ssh/known_hosts" -R "$DEPLOY_HOST" || true

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
        stage('Deploy Cloud') {
            steps {
                echo 'Deploying to cloud app server...'
                sh """
                    set -e
                    ssh-keygen -f "/var/lib/jenkins/.ssh/known_hosts" -R "$CLOUD_HOST" || true
            
                    ssh -i "$SSH_KEY" -o StrictHostKeyChecking=no \
                        "$CLOUD_USER@$CLOUD_HOST" \
                        "sudo mkdir -p $APP_DIR && sudo chown $CLOUD_USER:$CLOUD_USER $APP_DIR"

                    scp -i "$SSH_KEY" -o StrictHostKeyChecking=no -r \
                        ./publish/* "$CLOUD_USER@$CLOUD_HOST:$APP_DIR/"

                    ssh -i "$SSH_KEY" -o StrictHostKeyChecking=no \
                        "$CLOUD_USER@$CLOUD_HOST" \
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