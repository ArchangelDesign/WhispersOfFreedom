package com.whispersoffreedom.api.config;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Configuration;
import org.springframework.stereotype.Service;

@Service
@Configuration
public class ConfigurationProvider {

    @Value("${adminApiUrl}")
    private String adminApiUrl;

    public String getAdminApiUrl() {
        return adminApiUrl;
    }
}
