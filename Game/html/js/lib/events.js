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
                var _this = this;
                _.remove(callbacks, function(e) { return e === _this; });
            },
        };
    };
    
    return {
        UpdateMixin: makeEventMixin('update', 'onUpdate'),
        LobbyMixin: makeEventMixin('lobby', 'onLobbyUpdate'),
        OnChatMixin: makeEventMixin('addChatMessage', 'onChatMessage'),
        ShowUpdateMixin: makeEventMixin('show', 'onShowUpdate'),
        SetParamsMixin: makeEventMixin('setParams', 'onSetParams'),
        SetModeMixin: makeEventMixin('setMode', 'onSetMode'),
    };
});