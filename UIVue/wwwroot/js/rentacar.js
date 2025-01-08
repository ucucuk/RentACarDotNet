
var app = new Vue({
	el: "#app1",
	data: {
		baslik: "Kisiler",
		gorunsun: true,
		olussun: true,
		isimler: [{ isim: "Ulas" }, { isim: "Cucuk" }]
	},
	methods: {
		denemeClick: function () {
			this.gorunsun = !this.gorunsun;
		}
	}
});