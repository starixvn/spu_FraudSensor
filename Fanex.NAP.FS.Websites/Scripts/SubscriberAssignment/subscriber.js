function Subscriber(SubscriberID, SubscriberName, isAssigned) {
    this.id = SubscriberID;
    this.name = SubscriberName;
    this.isAssigned = isAssigned == 0 ? false : true;

    Subscriber.prototype.SetAssign = function(assign) {
        this.isAssigned = assign;
    }
}

$(Subscriber);