const PROXY_CONFIG = [
  {
    context: [
      "/data",
    ],
    target: "https://localhost:7139",
    secure: false
  }
]

module.exports = PROXY_CONFIG;
