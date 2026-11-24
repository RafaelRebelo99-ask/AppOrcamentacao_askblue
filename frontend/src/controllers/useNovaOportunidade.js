import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

export default function useNovaOportunidade() {
  const navigate = useNavigate();

  // Dados do formulário
  const [dados, setDados] = useState({
    idCliente: '',
    idCategoria: '',
    titulo: '',
    descricao: '',
    prazoProposta: '',
    dataPretendida: '',
  });

  // Dados provenientes do backend
  const [clientes, setClientes] = useState([]);
  const [categorias, setCategorias] = useState([]);

  // Estados da página
  const [loading, setLoading] = useState(false);
  const [erro, setErro] = useState('');

  // Atualizar campos do formulário
  const handleChange = (e) => {
    const { name, value } = e.target;

    setDados(prev => ({
      ...prev,
      [name]: value,
    }));
  };

  // Carregar clientes do backend
  const carregarClientes = async () => {
  };

  // Carregar categorias do backend
  const carregarCategorias = async () => {
  };

  // Registar oportunidade no backend
  const handleSubmit = async (e) => {
  };

  // Cancelar criação da oportunidade
  const handleCancelar = () => {
    navigate('/oportunidades');
  };

  return {
    dados,
    clientes,
    categorias,
    loading,
    erro,
    handleChange,
    handleSubmit,
    handleCancelar,
    carregarClientes,
    carregarCategorias,
  };
}