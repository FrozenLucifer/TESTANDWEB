CREATE TABLE products (
                          id SERIAL PRIMARY KEY,
                          name VARCHAR(100) NOT NULL,
                          price DECIMAL(10,2) NOT NULL,
                          category_id INT NOT NULL,
                          created_at TIMESTAMP DEFAULT NOW()
);

CREATE TABLE categories (
                            id SERIAL PRIMARY KEY,
                            name VARCHAR(50) NOT NULL
);

CREATE TABLE orders (
                        id SERIAL PRIMARY KEY,
                        product_id INT NOT NULL,
                        quantity INT NOT NULL,
                        total_price DECIMAL(10,2) NOT NULL,
                        created_at TIMESTAMP DEFAULT NOW()
);


ALTER SYSTEM SET max_connections = 1500;