const URL_API = 'http://localhost:5139/api/v1/Documentos';


async function listarDocumentos() {

    const codigoCliente = document.getElementById("codigoClienteBusca").value;

    if (!codigoCliente) {
        alert("Informe o código do cliente.");
        return;
    }

    const response = await fetch(`${URL_API}/listar/${codigoCliente}`);

    const corpoTabela = document.getElementById("corpoTabela");

    corpoTabela.innerHTML = "";

    if (response.ok) {

        const documentos = await response.json();

        documentos.forEach(documento => {

            const linha = document.createElement("tr");

            linha.innerHTML = `
                <td>${documento.id}</td>
                <td>${documento.name}</td>
                <td>${documento.extensao}</td>
                <td>
                    <button class="btn-baixar" onclick="baixarDocumento(${documento.id})">
                        Baixar
                    </button>

                    <button class="btn-excluir" onclick="excluirDocumento(${documento.id})">
                        Excluir
                    </button>
                </td>
            `;

            corpoTabela.appendChild(linha);
        });

    } else {

        alert("Nenhum documento encontrado.");
    }
}


async function baixarDocumento(id) {

    const response = await fetch(`${URL_API}/download/${id}`);

    if (!response.ok) {
        alert("Erro ao baixar o documento.");
        return;
    }

    const arquivo = await response.blob();

    const url = window.URL.createObjectURL(arquivo);

    const link = document.createElement("a");

    link.href = url;
    link.download = "";

    document.body.appendChild(link);

    link.click();

    link.remove();

    window.URL.revokeObjectURL(url);
}



async function excluirDocumento(id) {

    const confirmar = confirm("Deseja realmente excluir este documento?");

    if (!confirmar) {
        return;
    }

    const response = await fetch(`${URL_API}/excluir/${id}`, {
        method: "DELETE"
    });

    if (response.ok) {

        alert("Documento excluído com sucesso.");

        listarDocumentos();

    } else {

        alert("Erro ao excluir o documento.");
    }
}



async function enviarDocumento() {

    const codigoCliente = document.getElementById("codigoCliente").value;

    const inputArquivo = document.getElementById("arquivo");

    const arquivo = inputArquivo.files[0];


    if (!codigoCliente || !arquivo) {

        alert("Informe o código do cliente e selecione um arquivo");

        return;
    }


    const dadosArquivo = new FormData();

    dadosArquivo.append("arquivo", arquivo);


    const response = await fetch(`${URL_API}/uploud/${codigoCliente}`, {

        method: "POST",

        body: dadosArquivo

    });


    if (response.ok) {

        alert("Documento enviado com sucesso!");

        document.getElementById("codigoCliente").value = "";

        document.getElementById("arquivo").value = "";



        document.getElementById("codigoClienteBusca").value = codigoCliente;

        listarDocumentos();

    }
    else {

        alert("Erro: Falha ao enviar o Documento");
    }
}