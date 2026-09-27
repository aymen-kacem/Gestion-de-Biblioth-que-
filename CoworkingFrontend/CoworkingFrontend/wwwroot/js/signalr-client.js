(function () {
    if (window.signalRClient) return;

    window.signalRClient = {
        connections: {},

        createConnection: async function (key, hubUrl, accessToken) {
            if (!signalR) throw 'SignalR client not found. Make sure @microsoft/signalr is loaded.';
            if (this.connections[key]) return;

            const options = accessToken ? { accessTokenFactory: () => accessToken } : undefined;
            const builder = new signalR.HubConnectionBuilder()
                .withUrl(hubUrl, options)
                .withAutomaticReconnect();

            const connection = builder.build();

            this.connections[key] = {
                connection: connection,
                handlers: []
            };

            // optional logging
            connection.onclose(function (error) {
                console && console.debug && console.debug('SignalR connection closed', key, error);
            });
        },

        on: function (key, eventName, dotNetRef, methodName) {
            const entry = this.connections[key];
            if (!entry) throw `Connection '${key}' not found. Call createConnection first.`;

            const handler = function () {
                const args = Array.from(arguments);
                if (dotNetRef) {
                    // invoke method on .NET side
                    dotNetRef.invokeMethodAsync(methodName, ...args).catch(err => console && console.error && console.error(err));
                }
            };

            entry.connection.on(eventName, handler);
            entry.handlers.push({ eventName, handler });
        },

        start: async function (key) {
            const entry = this.connections[key];
            if (!entry) throw `Connection '${key}' not found. Call createConnection first.`;
            await entry.connection.start();
        },

        send: async function (key, methodName) {
            const entry = this.connections[key];
            if (!entry) throw `Connection '${key}' not found. Call createConnection first.`;
            const args = Array.prototype.slice.call(arguments, 2);
            return await entry.connection.invoke(methodName, ...args);
        },

        stop: async function (key) {
            const entry = this.connections[key];
            if (!entry) return;

            // remove handlers
            entry.handlers.forEach(h => {
                try { entry.connection.off(h.eventName, h.handler); } catch (e) { }
            });
            entry.handlers = [];

            try { await entry.connection.stop(); } catch (e) { }
            delete this.connections[key];
        }
    };
})();