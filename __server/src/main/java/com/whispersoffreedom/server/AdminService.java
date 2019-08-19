package com.whispersoffreedom.server;

import com.whispersoffreedom.api.config.ConfigurationProvider;
import com.whispersoffreedom.api.dto.Credentials;
import org.springframework.http.HttpEntity;
import org.springframework.http.HttpMethod;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestClientException;
import org.springframework.web.client.RestTemplate;

@Service
public class AdminService {

    private ConfigurationProvider config;

    public AdminService(ConfigurationProvider config) {
        this.config = config;
    }

    public boolean verifyCredentials(String email, String password) {
        RestTemplate rt = new RestTemplate();
        Credentials cred = new Credentials(email, password);
        ResponseEntity<String> response;
        try {
            response = rt.exchange(
                    config.getAdminApiUrl() + "auth/verify",
                    HttpMethod.POST,
                    new HttpEntity<>(cred),
                    String.class);
        } catch (RestClientException e) {
            return false;
        }
        return response.getStatusCode() == HttpStatus.OK;
    }
}
