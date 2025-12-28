#!/bin/bash
set -e

# Script to update image tags in helm values files
# Usage: ./scripts/update-image-tags.sh [backend|frontend|both] <tag>
# Example: ./scripts/update-image-tags.sh backend main-abc123

COMPONENT=${1:-both}
NEW_TAG=$2

if [ -z "$NEW_TAG" ]; then
    echo "Usage: $0 [backend|frontend|both] <tag>"
    echo "Example: $0 backend main-abc123"
    echo "Example: $0 both main-abc123"
    exit 1
fi

update_backend() {
    echo "Updating backend image tag to: $NEW_TAG"
    sed -i.bak "s|tag: \"main-.*\"|tag: \"$NEW_TAG\"|g" helm/backend/values.yaml
    rm -f helm/backend/values.yaml.bak
}

update_frontend() {
    echo "Updating frontend image tag to: $NEW_TAG"
    sed -i.bak "s|tag: \"main-.*\"|tag: \"$NEW_TAG\"|g" helm/frontend/values.yaml
    rm -f helm/frontend/values.yaml.bak
}

case $COMPONENT in
    backend)
        update_backend
        ;;
    frontend)
        update_frontend
        ;;
    both)
        update_backend
        update_frontend
        ;;
    *)
        echo "Invalid component: $COMPONENT"
        echo "Must be: backend, frontend, or both"
        exit 1
        ;;
esac

echo "Image tag(s) updated successfully!"
echo ""
echo "Changed files:"
git diff --name-only helm/*/values.yaml

echo ""
echo "To commit and deploy:"
echo "  git add helm/*/values.yaml"
echo "  git commit -m 'Update image tag to $NEW_TAG'"
echo "  git push origin main"
