use std::collections::HashMap;
use serde::{Serialize,Deserialize};
use std::io::{stdin,stdout,Write,Read};

#[derive(Deserialize)]
pub struct HttpRequest {
    pub path: String,
    pub method: String,
    pub body: Vec<u8>,
    pub headers: HashMap<String, Vec<String>>,
    pub query: HashMap<String, Vec<String>>,
}

#[derive(Serialize)]
pub struct HttpResponse {
    pub headers: HashMap<String, Vec<String>>,
    pub body: Vec<u8>,
}

pub fn get_http_request() -> HttpRequest {
    let mut request_json = String::new();
    let _ = stdout().flush();
    stdin().read_to_string(&mut request_json).expect("It was not possible to read from stdin");

    let request: HttpRequest = serde_json::from_str(&request_json).unwrap();
    request
}

pub fn write_response(text: &str, headers: &HashMap<String, Vec<String>>) {
    let response = HttpResponse {
        headers: headers.clone(),
        body: text.as_bytes().to_vec()
    };

    let response_json = serde_json::to_string(&response).expect("It was not possible to serialize response");
    
    println!("{}", &response_json);
}

pub fn write_json_response<T: Serialize>(value: &T, headers: &HashMap<String, Vec<String>>) {

    let mut headers_copy = headers.clone();
    headers_copy.insert("Content-Type".to_string(), ["application/json".to_string()].to_vec());

    let response_text = serde_json::to_string(&value).expect("It was not possible to serialize response");

    write_response(&response_text, &headers_copy);
}