use std::collections::HashMap;
use serde::{Serialize,Deserialize};

mod http;

#[derive(Deserialize)]
struct Person {
    name: String
}

#[derive(Serialize)]
struct Greeting {
    message: String
}

#[no_mangle]
pub extern "C" fn run() {
    let req = http::get_http_request();

    let body_utf8 = std::str::from_utf8(&req.body).expect("failed to ready body");

    let person: Person = serde_json::from_str(&body_utf8).expect("não sei");

    let person_result = Greeting {
        message: format!("Olá, {}", person.name)
    };

    let mut response_headers: HashMap<String, Vec<String>> = HashMap::new();
    response_headers.insert(String::from("Custom-Header"), vec![String::from("custom header value")]);

    http::write_json_response(&person_result, &response_headers);
}