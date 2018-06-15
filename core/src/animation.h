
#ifndef HEXBOT_ANIMATION
#define HEXBOT_ANIMATION

#include "api.h"
#include "utils.h"

typedef std::shared_ptr<class Animation> AnimationPtr;

class Animation
{
public:
    static AnimationPtr Create(const std::string& filename);

private:
    Animation(const std::string& filename);
    
protected:
    void read(const Json::Value& data);
    
private:
    
};

#endif
