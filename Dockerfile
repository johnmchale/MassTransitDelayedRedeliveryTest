FROM rabbitmq:3.12.12-management

# Copy the delayed message plugin file into the RabbitMQ plugins directory
COPY ./rabbitmq_delayed_message_exchange-3.12.0.ez /opt/rabbitmq/plugins/

# Enable the delayed message plugin
RUN rabbitmq-plugins enable --offline rabbitmq_delayed_message_exchange
