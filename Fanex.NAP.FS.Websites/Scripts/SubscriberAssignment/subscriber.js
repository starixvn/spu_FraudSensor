function Subscriber(SubscriberID, SubscriberName, isAssigned, isMaster) {
    this.id = SubscriberID;
    this.name = SubscriberName;
    this.isMaster = isMaster != null ? isMaster : false;
    this.isAssigned = isAssigned == 0 ? false : true;

    Subscriber.prototype.SetAssign = function(assign) {
        this.isAssigned = assign;
    }
}

$(Subscriber);