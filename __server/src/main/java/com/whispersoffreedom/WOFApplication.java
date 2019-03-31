package com.whispersoffreedom;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.scheduling.annotation.EnableScheduling;

@SpringBootApplication
@EnableScheduling
public class WOFApplication {

	public static void main(String[] args) {
		SpringApplication.run(WOFApplication.class, args);
	}

}
