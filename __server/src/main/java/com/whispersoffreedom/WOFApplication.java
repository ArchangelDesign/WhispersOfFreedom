package com.whispersoffreedom;

import com.whispersoffreedom.server.WofServer;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.scheduling.annotation.EnableScheduling;

import java.io.IOException;

@SpringBootApplication
@EnableScheduling
public class WOFApplication {

	static Logger logger = LoggerFactory.getLogger(WOFApplication.class);

	public static void main(String[] args) throws IOException {
		logger.info("Starting WOF server...");
		SpringApplication.run(WOFApplication.class, args);
		WofServer.initializeServer();
	}

}
