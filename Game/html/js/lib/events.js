define(['lodash'], function(_) {
    var makeEventMixin = function(triggerName, eventName) {
        var callbacks = [];
        
        window[triggerName] = function(json) {
            _.each(callbacks, function(item) {
                item[eventName](json);
            });
        };
        
        return {
            componentDidMount: function() {
                callbacks.push(this);
            },
            
            componentWillUnmount: function() {
                _.remove(callbacks, function(e) { e === this; });
            },
        };
    };
    
    return {
        UpdateMixin: makeEventMixin('update', 'onUpdate'),
        OnChatMixin: makeEventMixin('addChatMessage', 'onChatMessage'),
    };
});