
#include "animation.h"
#include "json/reader.h"
#include "utils.h"

AnimationPtr Animation::Create(const std::string& filename)
{
    return AnimationPtr(new Animation(filename));
}

Animation::Animation(const std::string& filename)
{
    std::ifstream t(filename);
    std::string str((std::istreambuf_iterator<char>(t)), std::istreambuf_iterator<char>());
    
    Json::Value root;
    if (!Json::Reader().parse(str, root))
    {
        std::cerr << "Failed to load animation " << filename << std::endl;
        abort();
    }
    
    read(root);
}


void Animation::read(const Json::Value& data)
{

}
