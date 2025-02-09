require 'jwt'
key_file = 'AuthKey_947J5TQ273.p8'
team_id = 'ZRRK6WN248'
client_id = 'com.metacore-ios'
key_id = '947J5TQ273'

ecdsa_key = OpenSSL::PKey::EC.new IO.read key_file
headers = {
	'kid' => key_id
}
claims = {
	'iss' => team_id,
	'iat' => Time.now.to_i,
	'exp' => Time.now.to_i + 86400 * 3650,
	'aud' => 'https://appleid.apple.com',
	'sub' => client_id
}
token = JWT.encode claims, ecdsa_key, 'ES256', headers
puts token
