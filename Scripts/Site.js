function mostraAviso(tipo, mensagem) {
    if (tipo === 'sucesso') {
        swal.fire({
            icon: 'success',
            title: mensagem,
            showConfirmButtom: false,
            timer: 3000
        });
    } else {
        swal.fire({
            icon: 'error',
            title: mensagem,
            showConfirmButtom: true
        });
    };
}