
var app = new Vue({
	el: "#app1",
	data: {
		baslik: "Markalar",
		gorunsun: true,
		olussun: true,
		isimler: [{ isim: "Ulas" }, { isim: "Cucuk" }],
		brands: [],
		yeniMarka: "",
		duzenlenenMarka: null
	},
	methods: {
		denemeClick: function () {
			this.gorunsun = !this.gorunsun;
		}
		, listele: function () {
			fetch("https://localhost:44335/api/Brands")
				.then(response => response.json())
				.then(data => { console.log(data); this.brands = data; });
		},
		yenimarkafunc: function () {
			const marka = { name: this.yeniMarka };
			fetch("https://localhost:44335/api/Brands", {
				method: 'POST',
				headers: {
					//'Authorization': 'Bearer ' + this.token,
					'Accept': 'application/json',
					'Content-Type': 'application/json;charset=utf-8'
				},
				body: JSON.stringify(marka)
			})
				.then(response => response.json())
				.then(data => {
					console.log('Success:', data);
					this.listele();
					this.yeniMarka = "";
				})
				.catch((error) => {
					console.error('Error:', error);
				});
		},
		markasilfunc: function (brand) {
			fetch("https://localhost:44335/api/Brands/" + brand.id, {
				method: 'DELETE',
				headers: {
					//'Authorization': 'Bearer ' + this.token,
					'Accept': 'application/json',
					'Content-Type': 'application/json;charset=utf-8'
				}
			})
				.then(data => {
					console.log('Success:', data);
					this.listele();
				})
				.catch((error) => {
					console.error('Error:', error);
				});
		},
		duzenlemarkafunc: function (duzenlenenMarka) {
			fetch("https://localhost:44335/api/Brands", {
				method: 'PUT',
				headers: {
					//'Authorization': 'Bearer ' + this.token,
					'Accept': 'application/json',
					'Content-Type': 'application/json;charset=utf-8'
				},
				body: JSON.stringify(duzenlenenMarka)
			})
				.then(data => {
					console.log('Success:', data);
					this.listele();
				})
				.catch((error) => {
					console.error('Error:', error);
				});
			this.duzenlenenMarka = null;
		},
	},
	created: function () {
		this.listele();
	}
});