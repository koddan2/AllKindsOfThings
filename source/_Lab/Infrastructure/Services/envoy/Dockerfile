FROM envoyproxy/envoy-dev:latest

ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get -qq update \
    && apt-get -qq install --no-install-recommends -y curl \
    && apt-get -qq autoremove -y \
    && apt-get clean \
    && rm -rf /tmp/* /var/tmp/* /var/lib/apt/lists/*
COPY ./frontproxy-envoy.yaml /etc/front-envoy.yaml
RUN chmod go+r /etc/front-envoy.yaml
CMD ["/usr/local/bin/envoy", "-c", "/etc/front-envoy.yaml", "--service-cluster", "front-proxy"]
